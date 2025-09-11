use leptos::{html::*, prelude::*};

pub fn login_button(url: &str) -> impl IntoView {
    form()
        .method("post")
        .action(format!("{url}/me/login"))
        .child(button().child("Login"))
}

pub fn logout_button(url: &str) -> impl IntoView {
    form()
        .method("post")
        .action(format!("{url}/me/logout"))
        .child(button().child("Logout"))
}
