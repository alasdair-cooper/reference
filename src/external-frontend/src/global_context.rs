use leptos::prelude::{expect_context, provide_context};
use reactive_stores::Store;

use crate::{api_client::ApiClient, signal_r_client::SignalRClient};

pub fn set_global_context(state: GlobalState) {
    provide_context(Store::new(state))
}

pub fn expect_global_context() -> Store<GlobalState> {
    expect_context::<Store<GlobalState>>()
}

#[derive(Clone, Debug, Store)]
pub struct GlobalState {
    pub api_client: ApiClient,
    pub signal_r_client: SignalRClient,
}
