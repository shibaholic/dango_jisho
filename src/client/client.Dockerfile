# Stage 1: Build the React app using Node
FROM node:18-alpine AS builder
WORKDIR /app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm install

# Copy the rest of the source code and build the project
COPY . .
RUN npm run build

# Stage 2: Serve the built files with Nginx
FROM nginx:stable-alpine
# Remove default Nginx configuration if needed
RUN rm /etc/nginx/conf.d/default.conf

# Copy custom Nginx configuration (optional)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy the build output from the builder stage
COPY --from=builder /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]