import logging

import requests
from flask import Flask, jsonify, request


# Configure logging
logging.basicConfig(level=logging.DEBUG)  # Set the logging level to DEBUG


app = Flask(__name__)

# User Management API endpoint
USER_API_URL = "http://localhost:5000/api/users"

# File Management API endpoint
FILE_API_URL = "http://localhost:5001/api/files"


@app.route("/api/users/register", methods=["POST"])
def register_user():
    try:
        # Forward the request to User Management API
        response = requests.post(f"{USER_API_URL}/register", json=request.get_json())
        return response.content, response.status_code
    except Exception as e:
        return jsonify(error=str(e)), 500


@app.route("/api/users/upload", methods=["POST"])
def upload_file():
    try:
        # Read the file and create a FileUploadRequest object
        file_path = 'D:/1.txt'  # Specify the correct file path
        with open(file_path, 'rb') as file:
            file_data = file.read()

        # Forward the request to User Management API's UploadFile endpoint
        response = requests.post(f"{USER_API_URL}/upload", data=file_data, headers={'Content-Type': 'application/octet-stream'})


        return response.content, response.status_code
    except Exception as e:
        # Log the exception
        logging.error(f"Error occurred: {str(e)}")
        return jsonify(error=str(e)), 500


@app.route("/api/users/status", methods=["GET"])
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
        return jsonify(combined_response), 200
    except Exception as e:
        return jsonify(error=str(e)), 500


if __name__ == "__main__":
    app.run(host="0.0.0.0", port=80)
