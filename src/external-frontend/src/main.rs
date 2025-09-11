mod api_client;
mod auth;
mod basket;
mod global_context;
mod signal_r_client;

use leptos::ev;
use log::Level;

use leptos::{html::*, prelude::*};

use crate::{
    api_client::{ApiClient, ItemDto},
    auth::{login_button, logout_button},
    basket::{add_item_to_basket_button, basket},
    global_context::{
        GlobalState, GlobalStateStoreFields, expect_global_context, set_global_context,
    },
    signal_r_client::SignalRClient,
};

const URL: &str = "https://localhost:7101";
const WS_URL: &str = "wss://localhost:7101";

fn main() {
    console_error_panic_hook::set_once();
    console_log::init_with_level(Level::Debug).unwrap();

    leptos::mount::mount_to_body(|| {
        div()
            .child(login_button(URL))
            .child(logout_button(URL))
            .child(load_page())
            .child(basket())
    })
}

fn load_page() -> impl IntoView {
    set_global_context(GlobalState {
        api_client: ApiClient::new(URL),
        signal_r_client: SignalRClient::new(WS_URL),
    });

    let (page, set_page) = signal(1);

    let client = expect_global_context().api_client();

    let items =
        LocalResource::new(move || async move { client.get().list_items(page.get(), 10).await });

    let prev_page = move || {
        if page.get() > 1 {
            set_page.update(move |x| {
                *x -= 1;
            });
        }
    };

    let next_page = move || {
        set_page.update(move |x| {
            *x += 1;
        });
    };

    move || match items.get() {
        Some(Ok(items)) => div()
            .child(items_list(&items))
            .child(prev_page_button(prev_page, page.get() == 1))
            .child(page.get())
            .child(next_page_button(next_page))
            .into_any(),
        Some(Err(x)) => p().child(format!("Load failed: {x}.")).into_any(),
        None => p().child("Loading...").into_any(),
    }
}

fn items_list(items: &Vec<ItemDto>) -> impl IntoView {
    ul().child(
        items
            .iter()
            .map(|x| {
                li().child(format!("{} - {}", x.sku_id, x.display_name))
                    .child(add_item_to_basket_button(x.sku_id))
            })
            .collect_view(),
    )
}

fn prev_page_button(on_click: impl Fn() + 'static, is_disabled: bool) -> impl IntoView {
    button()
        .child("Previous Page")
        .on(ev::click, move |_| on_click())
        .disabled(is_disabled)
}

fn next_page_button(on_click: impl Fn() + 'static) -> impl IntoView {
    button()
        .child("Next Page")
        .on(ev::click, move |_| on_click())
}
