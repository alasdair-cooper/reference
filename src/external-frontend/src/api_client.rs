use reqwest::Client;

#[derive(Clone, Debug)]
pub struct ApiClient {
    pub url: String,
    client: Client,
}

impl ApiClient {
    pub fn new(url: &str) -> Self {
        Self {
            url: url.into(),
            client: Client::new(),
        }
    }

    pub async fn list_items(&self, page: i32, page_size: i32) -> Result<Vec<ItemDto>, String> {
        self.client
            .get(format!(
                "{url}/items?page={page}&pageSize={page_size}",
                url = self.url
            ))
            .fetch_credentials_include()
            .send()
            .await
            .map(|x| x.json::<Vec<ItemDto>>())
            .map_err(|x| format!("{}", x))?
            .await
            .map_err(|x| format!("{}", x))
    }

    pub async fn add_item_to_basket(&self, sku_id: i32, count: i32) {
        self.client
            .post(format!("{url}/me/basket", url = self.url))
            .json(&AddItemToBasketRequest { sku_id, count })
            .fetch_credentials_include()
            .send()
            .await
            .unwrap();
    }

    pub async fn update_item_in_basket(&self, sku_id: i32, count: i32) {
        self.client
            .put(format!("{url}/me/basket/{sku_id}", url = self.url))
            .json(&UpdateItemInBasketRequest { count })
            .fetch_credentials_include()
            .send()
            .await
            .unwrap();
    }

    pub async fn remove_item_from_basket(&self, sku_id: i32) {
        self.client
            .delete(format!("{url}/me/basket/{sku_id}", url = self.url))
            .fetch_credentials_include()
            .send()
            .await
            .unwrap();
    }

    pub async fn list_basket_items(&self) -> Result<Vec<BasketItemDto>, String> {
        self.client
            .get(format!("{url}/me/basket", url = self.url))
            .fetch_credentials_include()
            .send()
            .await
            .map(|x| x.json::<Vec<BasketItemDto>>())
            .map_err(|x| format!("{}", x))?
            .await
            .map_err(|x| format!("{}", x))
    }
}

#[derive(serde::Deserialize, Clone)]
#[serde(rename_all(deserialize = "camelCase"))]
pub struct ItemDto {
    pub sku_id: i32,
    pub display_name: String,
}

#[derive(serde::Deserialize, Clone)]
#[serde(rename_all(deserialize = "camelCase"))]
pub struct BasketItemDto {
    pub sku_id: i32,
    pub display_name: String,
    pub count: i32,
}

#[derive(serde::Serialize)]
#[serde(rename_all(serialize = "camelCase"))]
pub struct UpdateItemInBasketRequest {
    count: i32,
}

#[derive(serde::Serialize)]
#[serde(rename_all(serialize = "camelCase"))]
pub struct AddItemToBasketRequest {
    sku_id: i32,
    count: i32,
}
