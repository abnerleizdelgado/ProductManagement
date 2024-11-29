FROM node:18

WORKDIR /app

COPY package*.json ./

RUN rm -rf node_modules package-lock.json \
    && npm install

COPY . .

EXPOSE 5174

ENV HOST=0.0.0.0

CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0"]
