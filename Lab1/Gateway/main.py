from flask import Flask, request, jsonify
import requests

app = Flask(__name__)

# Define the base URLs for your microservices
USER_SERVICE_BASE_URL = "http://user-service:5000"  # Replace with your actual user service URL
FILE_SERVICE_BASE_URL = "http://file-service:5001"  # Replace with your actual file service URL


@app.route("/api/users", methods=["POST"])
def handle_user_requests():
    # Forward the request to the User Service
    response = requests.post(f"{USER_SERVICE_BASE_URL}/api/users", json=request.json)
    return jsonify(response.json()), response.status_code


@app.route("/api/files/upload", methods=["POST"])
def handle_file_upload():
    # Forward the request to the File Service
    response = requests.post(f"{FILE_SERVICE_BASE_URL}/api/files/upload", json=request.json)
    return jsonify(response.json()), response.status_code


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=80)  # Run the gateway on port 80
