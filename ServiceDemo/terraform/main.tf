# Local variables for resource tagging
locals {
  common_tags = {
    "Created On"  = formatdate("DD MMM YYYY hh:mm ZZZ",timestamp())
  }
}

resource "azurerm_resource_group" "servicedemorg" {
  name     = "ServiceDemoRG"
  location = var.location
}

# create application insights for servicedemo_api
resource "azurerm_application_insights" "servicedemo_api_insights" {
  name                = "${var.prefix}-servicedemo-api"
  location            = var.location
  resource_group_name = azurerm_resource_group.servicedemorg.name
  application_type    = "web"
  retention_in_days   = 90
}

# create resource for servicedemo_api
resource "azurerm_app_service" "servicedemo_api" {
  name                = "${var.prefix}-servicedemo-api"
  location            = var.location
  resource_group_name = azurerm_resource_group.servicedemorg.name
  app_service_plan_id = data.azurerm_app_service_plan.plan.id

  client_affinity_enabled = false

  site_config {
    default_documents        = ["index.htm", "index.html", "hostingstart.html", "Default.htm", "Default.html"]
    health_check_path        = "/api/healthcheck"
    linux_fx_version         = "DOTNETCORE|3.1"
  }

  app_settings = {
    APPINSIGHTS_INSTRUMENTATIONKEY                = join("", azurerm_application_insights.servicedemo_api_insights.*.instrumentation_key)
    WEBSITE_RUN_FROM_PACKAGE                      = "1"
    SCM_DO_BUILD_DURING_DEPLOYMENT                = false
    ASPNETCORE_ENVIRONMENT                        = "Production"
  }

  lifecycle {
    ignore_changes = [tags]
  }
  tags = local.common_tags
}