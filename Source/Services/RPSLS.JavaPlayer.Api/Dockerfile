FROM openjdk:11-jre AS base
WORKDIR /app

FROM openjdk:11-jdk AS maven
WORKDIR /src
COPY . .
RUN chmod +x ./mvnw
RUN ./mvnw package

FROM base as final
WORKDIR /app
COPY --from=maven /src/target/java.player-0.0.1-SNAPSHOT.jar .
EXPOSE 8080
ENTRYPOINT exec java -Djava.security.egd=file:/dev/./urandom -jar /app/java.player-0.0.1-SNAPSHOT.jar
