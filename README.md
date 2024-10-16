# warehouse-BE
 
# Quick Guide to Pull and Run Docker Image

## Step 1: Install Docker
Ensure you have Docker installed on your machine. Download from [Docker Desktop](https://www.docker.com/products/docker-desktop).

## Step 2: Open Terminal
Open your terminal (cmd, PowerShell, or any terminal you prefer).

## Step 3: Pull the Docker Image
Run the following command to pull the image from Docker Hub:
docker pull cevan23/warehousebeweb

## Step 4: Run the Docker Image
After pulling the image, run it with this command:
docker run -d -p 8080:8080 -p 8081:8081 cevan23/warehousebeweb

## Step 5: Access the Application
Open your browser and go to:

- **URL**: [http://localhost:8080](http://localhost:8080)

## Note
- To stop the running container, find its ID with `docker ps` and use `docker stop <container_id>`.
