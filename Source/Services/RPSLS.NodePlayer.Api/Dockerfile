FROM node:11-alpine

RUN mkdir -p /usr/src/app

WORKDIR /usr/src/app

COPY . .

RUN npm install
RUN npm prune --production
EXPOSE 3000
ENV PORT=3000

CMD ["npm", "run", "start"]