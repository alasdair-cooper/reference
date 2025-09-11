export function openDialog(selector) {
    document.querySelector(selector).showModal();
}

export function closeDialog(selector) {
    document.querySelector(selector).close();
}