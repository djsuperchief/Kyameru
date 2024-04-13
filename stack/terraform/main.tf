# Main tf entry

terraform {
    backend "s3" {
    bucket = "tfstate"
    key    = "local.tfstate"
    region = "eu-west-2"
    endpoint                    = "http://localstack-main:4566"
    skip_credentials_validation = true
    skip_metadata_api_check     = true
    force_path_style            = true
    #dynamodb_table              = "terraform_state"
    #dynamodb_endpoint           = "http://localhost:4566"
    #encrypt                     = true
    access_key                  = "mock_access_key"
    secret_key                  = "mock_secret_key"
    skip_requesting_account_id = true
  }
}

provider "aws" {

  access_key = "test"
  secret_key = "test"
  region     = "eu-west-2"

  s3_use_path_style           = true
  skip_credentials_validation = true
  skip_metadata_api_check     = true
  skip_requesting_account_id  = true

  endpoints {
    s3             = "http://localstack-main:4566"
  }
}

# Components
module "component_to" {
    source = "./components/s3"
}