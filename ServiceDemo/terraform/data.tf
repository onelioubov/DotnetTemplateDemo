data "azurerm_app_service_plan" "plan" {
  name                = "TemplateDemo-ASP"
  resource_group_name = "TemplateDemoRG"
}

data "azurerm_subscription" "sub" {
}