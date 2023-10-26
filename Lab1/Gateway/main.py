import logging
import requests
from flask import Flask, jsonify, request
from flask_caching import Cache

app = Flask(__name__)
cache = Cache(app, config={'CACHE_TYPE': 'simple'})  # Use an in-memory cache

# Configure logging
logging.basicConfig(level=logging.DEBUG)  # Set the logging level to DEBUG

# User Management API endpoint
USER_API_URL = "http://localhost:5000/api/users"

# File Management API endpoint
FILE_API_URL = "http://localhost:5001/api/files"

# Set cache timeout in seconds (e.g., 60 seconds)
CACHE_TIMEOUT = 60


@app.route("/api/users/register", methods=["POST"])
@cache.cached(timeout=CACHE_TIMEOUT)  # Cache the response of this endpoint for CACHE_TIMEOUT seconds
def register_user():
    try:
        # Forward the request to User Management API
        response = requests.post(f"{USER_API_URL}/register", json=request.get_json())
        return response.content, response.status_code  # Return the API response and status code
    except Exception as e:
        return jsonify(error=str(e)), 500  # Return a JSON error response with status code 500 if an exception occurs


@app.route("/api/users/upload", methods=["POST"])
@cache.cached(timeout=CACHE_TIMEOUT)  # Cache the response of this endpoint for CACHE_TIMEOUT seconds
def upload_file():
    try:
        # Read the file and create a FileUploadRequest object
        file_path = 'D:/1.txt'  # Specify the correct file path
        with open(file_path, 'rb') as file:
            file_data = file.read()

        # Forward the request to User Management API's UploadFile endpoint
        response = requests.post(f"{USER_API_URL}/upload", data=file_data, headers={'Content-Type': 'application/octet-stream'})

        return response.content, response.status_code  # Return the API response and status code
    except Exception as e:
        # Log the exception
        logging.error(f"Error occurred: {str(e)}")
        return jsonify(error=str(e)), 500  # Return a JSON error response with status code 500 if an exception occurs


@app.route("/api/users/status", methods=["GET"])
@cache.cached(timeout=CACHE_TIMEOUT)  # Cache the response of this endpoint for CACHE_TIMEOUT seconds
def get_status():
    try:
        # Forward the request to both microservices and return combined status
        user_api_response = requests.get(f"{USER_API_URL}/status")
        file_api_response = requests.get(f"{FILE_API_URL}/status")

        # Combine responses and return
        combined_response = {
            "user_api_status": user_api_response.json(),
            "file_api_status": file_api_response.json()
        }
        return jsonify(combined_response), 200  # Return a JSON response with status code 200
    except Exception as e:
        return jsonify(error=str(e)), 500  # Return a JSON error response with status code 500 if an exception occurs


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=80)  # Run the Flask app on host 0.0.0.0 and port 80
