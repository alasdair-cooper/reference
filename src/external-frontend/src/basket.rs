use leptos::{ev, html::*, prelude::*};

use crate::{
    GlobalStateStoreFields, api_client::BasketItemDto, global_context::expect_global_context,
};

pub fn add_item_to_basket_button(sku_id: i32) -> impl IntoView {
    let client = expect_global_context().api_client();

    let add_item_to_basket = Action::new_local(move |sku_id: &i32| {
        let sku_id = *sku_id;
        async move {
            client.get_untracked().add_item_to_basket(sku_id, 1).await;
        }
    });

    button()
        .on(ev::click, move |_| {
            add_item_to_basket.dispatch(sku_id);
        })
        .child("Add to basket")
}

pub fn basket() -> impl IntoView {
    let api_client = expect_global_context().api_client();
    let signal_r_client = expect_global_context().signal_r_client();
    let conn = signal_r_client.get_untracked().connect("personal");

    let basket_items =
        LocalResource::new(
            move || async move { api_client.get_untracked().list_basket_items().await },
        );

    conn.on_invocation("BasketUpdated", move |_| basket_items.refetch());

    move || match basket_items.get() {
        Some(Ok(items)) => ul()
            .child(items.iter().map(basket_item).collect_view())
            .into_any(),
        Some(Err(x)) => p().child(format!("Load failed: {x}.")).into_any(),
        None => p().child("Loading...").into_any(),
    }
}

fn basket_item(item: &BasketItemDto) -> impl IntoView {
    let sku_id = item.sku_id;
    let current_count = item.count;

    let api_client = expect_global_context().api_client();

    let on_increment = Action::new_local(move |_: &()| async move {
        api_client
            .get_untracked()
            .update_item_in_basket(sku_id, current_count + 1)
            .await
    });

    let on_decrement = Action::new_local(move |_: &()| async move {
        api_client
            .get_untracked()
            .update_item_in_basket(sku_id, current_count - 1)
            .await
    });

    let on_remove = Action::new_local(move |_: &()| async move {
        api_client
            .get_untracked()
            .remove_item_from_basket(sku_id)
            .await
    });

    li().child(format!("{} - {}", item.display_name.clone(), current_count))
        .child(decrement_item_in_basket_button(move || {
            on_decrement.dispatch(());
        }))
        .child(increment_item_in_basket_button(move || {
            on_increment.dispatch(());
        }))
        .child(remove_item_from_basket_button(move || {
            on_remove.dispatch(());
        }))
}

fn decrement_item_in_basket_button(on_click: impl Fn() + 'static) -> impl IntoView {
    button().child("-").on(ev::click, move |_| on_click())
}

fn increment_item_in_basket_button(on_click: impl Fn() + 'static) -> impl IntoView {
    button().child("+").on(ev::click, move |_| on_click())
}

fn remove_item_from_basket_button(on_click: impl Fn() + 'static) -> impl IntoView {
    button()
        .child("Remove All")
        .on(ev::click, move |_| on_click())
}
