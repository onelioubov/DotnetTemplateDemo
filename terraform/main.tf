# Local variables for resource tagging
locals {
  common_tags = {
    "Created On"  = formatdate("DD MMM YYYY hh:mm ZZZ",timestamp())
  }
}

resource "azurerm_resource_group" "dotnettemplatedemorg" {
  name     = "DotnetTemplateDemoRG"
  location = var.location
}

# create application insights for reference-store
resource "azurerm_application_insights" "dotnettemplatedemo_api_insights" {
  name                = "${var.prefix}-dotnettemplatedemo-api"
  location            = var.location
  resource_group_name = azurerm_resource_group.dotnettemplatedemorg.name
  application_type    = "web"
  retention_in_days   = 90
}

# create resource for reference-store
resource "azurerm_app_service" "dotnettemplatedemo_api" {
  name                = "${var.prefix}-dotnettemplatedemo-api"
  location            = var.location
  resource_group_name = azurerm_resource_group.dotnettemplatedemorg.name
  app_service_plan_id = data.azurerm_app_service_plan.plan.id

  client_affinity_enabled = false

  site_config {
    default_documents        = ["index.htm", "index.html", "hostingstart.html", "Default.htm", "Default.html"]
    scm_type                 = "LocalGit"
    health_check_path        = "/api/healthcheck"
  }

  app_settings = {
    APPINSIGHTS_INSTRUMENTATIONKEY                = join("", azurerm_application_insights.dotnettemplatedemo_api_insights.*.instrumentation_key)
    WEBSITE_RUN_FROM_PACKAGE                      = "1"
  }

  lifecycle {
    ignore_changes = [tags]
  }
  tags = local.common_tags
}