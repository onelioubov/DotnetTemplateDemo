data "azurerm_app_service_plan" "plan" {
  name                = "BuffaloWebDev-Demo"
  resource_group_name = "TemplateDemoRG"
}