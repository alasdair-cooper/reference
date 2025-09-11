use log::info;
use serde::{Deserialize, Serialize};
use thiserror::Error;
use wasm_bindgen::{JsCast, prelude::Closure};
use web_sys::{MessageEvent, WebSocket, js_sys::JsString};

#[derive(Clone, Debug)]
pub struct SignalRClient {
    pub url: String,
}

impl SignalRClient {
    pub fn new(url: &str) -> Self {
        Self {
            url: url.to_string(),
        }
    }

    // See https://stackoverflow.com/a/74016163/8853235
    pub fn connect(&self, endpoint: &str) -> SignalRConnection {
        let socket =
            WebSocket::new(format!("{}/{}", self.url.as_str(), endpoint).as_str()).unwrap();
        let on_connect = {
            let socket = socket.clone();
            Closure::<dyn Fn()>::new(move || {
                info!("[wss] connected");
                socket
                    .send_with_str(
                        format!(
                            r#"{{"protocol":"json","version":1}}{}"#,
                            JsString::from_char_code1(30)
                        )
                        .as_str(),
                    )
                    .unwrap();
            })
        };
        let on_disconnect = Closure::<dyn Fn()>::new(move || {
            info!("[wss] disconnected");
        });

        socket
            .add_event_listener_with_callback("open", on_connect.as_ref().unchecked_ref())
            .unwrap();
        socket
            .add_event_listener_with_callback("close", on_disconnect.as_ref().unchecked_ref())
            .unwrap();

        on_connect.forget();
        on_disconnect.forget();

        SignalRConnection { socket }
    }
}

pub struct SignalRConnection {
    socket: WebSocket,
}

impl SignalRConnection {
    pub fn on_message<F: Fn(SignalRResponse) + 'static>(&self, callback: F) {
        let boxed = Box::new(callback);
        let callback = {
            Closure::wrap(Box::new(move |ev: MessageEvent| {
                let str = ev.data().as_string().unwrap();
                let mut chars = str.chars();
                chars.next_back();
                let server_data = chars.as_str();

                if server_data != "{}" {
                    let data: SignalRResponse = serde_json::from_str(server_data).unwrap();
                    boxed(data);
                }
            }) as Box<dyn Fn(_)>)
        };
        self.socket
            .add_event_listener_with_callback("message", callback.as_ref().unchecked_ref())
            .unwrap();

        callback.forget();
    }

    pub fn on_invocation<F: Fn(Vec<String>) + 'static>(&self, target: &str, callback: F) {
        let target = target.to_string();
        self.on_message(move |x| {
            if let SignalRResponse::Invocation {
                r#type: _,
                target: tg,
                arguments: args,
            } = x
                && tg == target
            {
                callback(args);
            }
        });
    }

    pub fn _invoke(&self, target: &str, args: impl IntoIterator<Item = String>) {
        let req = SignalRResponse::Invocation {
            r#type: Version::<1>,
            target: target.to_string(),
            arguments: args.into_iter().collect(),
        };

        self.socket
            .send_with_str(serde_json::to_string(&req).unwrap().as_str())
            .unwrap();
    }
}

#[derive(Serialize, Deserialize, Debug)]
#[serde(untagged)]
pub enum SignalRResponse {
    Invocation {
        #[allow(dead_code)]
        r#type: Version<1>,
        #[allow(dead_code)]
        target: String,
        #[allow(dead_code)]
        arguments: Vec<String>,
    },
    StreamItem {
        #[allow(dead_code)]
        r#type: Version<2>,
    },
    Completion {
        #[allow(dead_code)]
        r#type: Version<3>,
    },
    StreamInvocation {
        #[allow(dead_code)]
        r#type: Version<4>,
    },
    CancelInvocation {
        #[allow(dead_code)]
        r#type: Version<5>,
    },
    Ping {
        #[allow(dead_code)]
        r#type: Version<6>,
    },
    Close {
        #[allow(dead_code)]
        r#type: Version<7>,
    },
}

#[derive(Debug)]
pub struct Version<const V: u8>;

#[derive(Debug, Error)]
#[error("Invalid version")]
struct VersionError;

impl<const V: u8> Serialize for Version<V> {
    fn serialize<S>(&self, serializer: S) -> Result<S::Ok, S::Error>
    where
        S: serde::Serializer,
    {
        serializer.serialize_u8(V)
    }
}

impl<'de, const V: u8> Deserialize<'de> for Version<V> {
    fn deserialize<D>(deserializer: D) -> Result<Self, D::Error>
    where
        D: serde::Deserializer<'de>,
    {
        let value = u8::deserialize(deserializer)?;
        if value == V {
            Ok(Version::<V>)
        } else {
            Err(serde::de::Error::custom(VersionError))
        }
    }
}
