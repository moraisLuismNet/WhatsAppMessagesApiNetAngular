#!/bin/bash
# Start Docker containers and open browser

DETECTED_OS="$(uname -s)"

# Detect Docker Compose command
if docker compose version &>/dev/null; then
  DOCKER_COMPOSE="docker compose"
elif command -v docker-compose &>/dev/null; then
  DOCKER_COMPOSE="docker-compose"
else
  echo "Error: neither 'docker compose' nor 'docker-compose' is available." >&2
  exit 1
fi

echo "========================================="
echo " Building Docker images"
echo " (may take 3-8 minutes on first run)"
echo "========================================="

$DOCKER_COMPOSE build || { echo "Build failed. Check errors above."; exit 1; }
echo "Images built successfully."

echo "Starting containers..."
$DOCKER_COMPOSE up -d --no-build

echo "Waiting for services to be ready..."
sleep 15

echo "Opening browser..."
case "$DETECTED_OS" in
  Linux*)
    xdg-open http://localhost:4200/ ;;
  Darwin*)
    open http://localhost:4200/ ;;
  CYGWIN*|MINGW*|MSYS*)
    start http://localhost:4200/ || explorer.exe "http://localhost:4200/" ;;
  *)
    echo "Open http://localhost:4200/ in your browser" ;;
esac
