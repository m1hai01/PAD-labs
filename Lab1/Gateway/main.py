from flask import Flask, jsonify, request
import requests
import os
from pybreaker import CircuitBreaker, CircuitBreakerError

app = Flask(__name__)

# Access environment variables
um1_url = os.environ.get('UM1', 'http://localhost:5000')
um2_url = os.environ.get('UM2', 'http://localhost:6000')
fm1_url = os.environ.get('FM1', 'http://localhost:5001')
fm2_url = os.environ.get('FM2', 'http://localhost:6001')

# Create a list of URLs for User Management API and File Management API
um_urls = [um1_url, um2_url]
fm_urls = [fm1_url, fm2_url]

# Initialize counters for round-robin indexing
um_counter = 0
fm_counter = 0

# Initialize Circuit Breaker for User Management API
um_circuit_breaker = CircuitBreaker(fail_max=3, reset_timeout=30)

# Initialize Circuit Breaker for File Management API
fm_circuit_breaker = CircuitBreaker(fail_max=3, reset_timeout=30)

# Function to perform round-robin selection for User Management API
@um_circuit_breaker
def get_um_url():
    global um_counter
    um_url = um_urls[um_counter]
    um_counter = (um_counter + 1) % len(um_urls)
    return um_url

# Function to perform round-robin selection for File Management API
@fm_circuit_breaker
def get_fm_url():
    global fm_counter
    fm_url = fm_urls[fm_counter]
    fm_counter = (fm_counter + 1) % len(fm_urls)
    return fm_url

# Route for testing circuit breaker
@app.route("/api/users/test_breaker", methods=["GET"])
def test_breaker():
    try:
        # Call the TestBreaker endpoint of User Management Service
        um_test_breaker_url = get_um_url() + "/api/users/test_breaker"
        response = requests.get(um_test_breaker_url)

        # Check the status code of the User Management Service response
        if response.status_code == 200:
            print(response)
            content = response.content
            status_code = response.status_code
            return content, status_code
        else:
            print("circuit breaker open")
            raise CircuitBreakerError(f"User Management Service returned status code {response.status_code}")

    except CircuitBreakerError as e:
        return redirect_request("/api/users/test_breaker")

# Function to test the circuit breaker
@um_circuit_breaker
def test_circuit_breaker():
    # Simulate a function that might fail
    if um_counter % 2 == 0:
        raise CircuitBreakerError("Simulated error")
    return {"status": "success"}

# Route for registering a user
@app.route("/api/users/register", methods=["POST"])
def register_user():
    try:
        # Forward the request to User Management API using round-robin
        user_api_url = get_um_url()
        response = requests.post(f"{user_api_url}/api/users/register", json=request.get_json())
        return response.content, response.status_code
    except CircuitBreakerError as e:
        return jsonify(error=str(e)), 500

# Route for uploading a file
@app.route("/api/users/upload", methods=["POST"])
def upload_file():
    try:
        # Forward the request to File Management API using round-robin
        file_api_url = get_fm_url()
        file_path = 'D:/1.txt'  # Specify the correct file path
        with open(file_path, 'rb') as file:
            file_data = file.read()
        response = requests.post(f"{file_api_url}/api/users/upload", data=file_data, headers={'Content-Type': 'application/octet-stream'})
        return response.content, response.status_code
    except CircuitBreakerError as e:
        # logging.error(f"Error occurred: {str(e)}")
        return jsonify(error=str(e)), 500

# Helper function to redirect request to another instance
def redirect_request(endpoint):
    new_instance_url = get_um_url()  # You can modify this to use another instance
    response = requests.get(f"{new_instance_url}{endpoint}")
    return response.content, response.status_code

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=80)
