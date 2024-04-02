resource "aws_s3_bucket" "component_to_s3" {
    bucket = "kyameru-component-s3"
    tags = {
        Name = "Kyameru component S3 to bucket"
        Environment = "local"
    }
}