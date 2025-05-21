resource "aws_ses_email_identity" "kyameru" {
    email = "kyameru@kyameru.com"
}

resource "aws_sqs_queue" "kyameru_ses_sender" {
  name = "kyameru-ses-sender"
  delay_seconds = 10
  visibility_timeout_seconds = 30
  max_message_size = 2048
  message_retention_seconds = 86400
  receive_wait_time_seconds = 2
  sqs_managed_sse_enabled = true
}
