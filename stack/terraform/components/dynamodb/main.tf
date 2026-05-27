resource "aws_dynamodb_table" "kyameru_from" {
    name = "KyameruTestFrom"
    billing_mode = "PROVISIONED"
    read_capacity = 20
    write_capacity = 20
    hash_key = "Identity"
    stream_enabled = true
    stream_view_type = "NEW_AND_OLD_IMAGES"

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