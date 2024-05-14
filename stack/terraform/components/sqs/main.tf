resource "aws_sqs_queue" "kyameru_to" {
  name = "kyameru-to"
  delay_seconds = 10
  visibility_timeout_seconds = 30
  max_message_size = 2048
  message_retention_seconds = 86400
  receive_wait_time_seconds = 2
  sqs_managed_sse_enabled = true
}

resource "aws_sqs_queue" "kyameru_from" {
  name = "kyameru-from"
  delay_seconds = 10
  visibility_timeout_seconds = 30
  max_message_size = 2048
  message_retention_seconds = 86400
  receive_wait_time_seconds = 2
  sqs_managed_sse_enabled = true
}

resource "aws_sqs_queue" "kyameru_sns_to" {
  name = "kyameru-sns-to"
  delay_seconds = 10
  visibility_timeout_seconds = 30
  max_message_size = 2048
  message_retention_seconds = 86400
  receive_wait_time_seconds = 2
  sqs_managed_sse_enabled = true
}