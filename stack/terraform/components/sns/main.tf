resource "aws_sns_topic" "kyameru_to" {
    name = "kyameru_to"
}

output "sns_kyameru_to_arn" {
    value = aws_sns_topic.kyameru_to.arn
}