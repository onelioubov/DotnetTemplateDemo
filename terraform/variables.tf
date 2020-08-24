// ***** Default Variables *****

variable "prefix" {
  description = "Custom prefix for the application services"
  type        = string
  default     = "service"
}

variable "location" {
  description = "The Azure Region in which the resource will be created."
  type        = string
  default     = "East US"
}