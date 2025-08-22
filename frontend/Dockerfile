FROM node:22-alpine AS build
WORKDIR /app

# Only copy package.json and package-lock.json for dependency install (no node_modules)
COPY package.json package-lock.json ./

# Remove lock and node_modules to workaround npm optional dependency bug, then install
RUN rm -rf package-lock.json node_modules && npm install

# Copy the rest of the app
COPY . .

# Build the Svelte app
RUN npm run build

# Expose the preview port
EXPOSE 5173

# Run the preview server
CMD ["npm", "run", "preview", "--", "--port", "5173", "--host", "0.0.0.0"]