docker build --pull --rm -f "Dockerfile" -t azureaifromtheorytopractice:latest .

docker run --rm -it -p 5000:5000/tcp --env-file .env azureaifromtheorytopractice:latest
