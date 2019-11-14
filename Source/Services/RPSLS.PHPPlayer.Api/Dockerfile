FROM composer:latest as dependencymanager
COPY ./src .
RUN composer install
 
FROM php:7.3.2-apache-stretch
COPY --chown=www-data:www-data ./src /srv/app
COPY ./src/.apache/vhost.conf /etc/apache2/sites-available/000-default.conf 
WORKDIR /srv/app
COPY --from=dependencymanager /app/vendor ./vendor
RUN docker-php-ext-install mbstring \
 && a2enmod rewrite negotiation \
 && docker-php-ext-install opcache

