FROM hashicorp/terraform:latest
COPY tf.sh ./
RUN chmod +x ./tf.sh
WORKDIR /terraform
#COPY ./terraform .


ENTRYPOINT [ "/tf.sh" ]