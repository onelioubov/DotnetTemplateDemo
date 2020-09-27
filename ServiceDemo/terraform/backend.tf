terraform {
  backend "azurerm" {
    resource_group_name  = "TemplateDemoRG"
    storage_account_name = "templatedemostate"
    container_name       = "states"
    key                  = "servicedemo.tfstate"
  }
}