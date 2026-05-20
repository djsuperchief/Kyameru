resource "aws_dynamodb_table" "kyameru_from" {
    name = "KyameruTestFrom"
    billing_mode = "PROVISIONED"
    read_capacity = 20
    write_capacity = 20
    hash_key = "Identity"

    attribute {
        name = "Identity"
        type = "S"
    }

    ttl {
        attribute_name = "TTL"
        enabled = true
    }

    tags = {
        Name = "dynamodb-tests"
    }
}
