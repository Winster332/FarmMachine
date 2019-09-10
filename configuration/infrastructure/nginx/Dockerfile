FROM nginx:1.13.6-alpine

## Remove default nginx website
RUN rm -rf /usr/share/nginx/html/*

## Copy our default nginx config
COPY default.conf /etc/nginx/conf.d/


CMD ["nginx", "-g", "daemon off;"]
