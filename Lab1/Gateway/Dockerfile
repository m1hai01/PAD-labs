FROM python:3.7.9
# Or any preferred Python version.
# Set the working directory in the container
WORKDIR /app

# Copy the current directory contents into the container at /app
ADD . /app

# Install required packages
RUN pip install Flask requests Flask-Caching

# Expose the port the app runs on
EXPOSE 80

# Run the application
CMD ["python", "main.py"]