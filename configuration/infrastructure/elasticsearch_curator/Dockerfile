# Docker Definition for ElasticSearch Curator

FROM python:2.7.8-slim
LABEL maintainer="rick.zakharov@gmail.com"

COPY config.yml config.yml
COPY action.yml action.yml

RUN pip install --quiet elasticsearch-curator

ENTRYPOINT ["/usr/local/bin/curator"]
