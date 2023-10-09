# Project: File Sharing System

### Application Suitability


&ensp;&ensp;&ensp;A File Sharing System like Google Drive is well-suited for a microservices architecture due to several reasons:

* Reliability. Reliability is extremely important for a storage system. Data loss is
unacceptable.
* Fast sync speed. If file sync takes too much time, users will become impatient and
abandon the product.
* Bandwidth usage. If a product takes a lot of unnecessary network bandwidth, users will
be unhappy, especially when they are on a mobile data plan.
* Scalability. The system should be able to handle high volumes of traffic.
* High availability. Users should still be able to use the system when some servers are
offline, slowed down, or have unexpected network errors.

Real-world examples of projects employing microservices include:

* Dropbox: Dropbox uses a microservices architecture to handle various components like user authentication, file synchronization, and sharing. Each of these components can be independently updated and scaled.

* Google Drive: While Google Drive's architecture is not public, it's widely believed that Google uses microservices extensively throughout their ecosystem, including Google Drive. The benefits of microservices align well with their need for scalability, fault tolerance, and independent development.

### Define Service Boundaries

**System architecture diagram**


![image](https://cdn.discordapp.com/attachments/826165651306971166/1156879263035818074/Blank_diagram.png?ex=65169346&is=651541c6&hm=de0e3224c58bf5f9b2354b7d4e2b96858d58bf48132a462e1c78cc7b192e1f71&)


 **System Components Overview**
 

- **User**: A user interacts with the application via a browser or a mobile app.

- **API Gateway**:Route incoming requests to the appropriate microservices based on the URL path or headers.Handle load balancing by distributing incoming requests evenly among API servers.

- **Service Discovery**: The Service Discovery mechanism helps us know where each instance is located. In this way, a Service Discovery component acts as a registry in which the addresses of all instances are tracked.


- **Block Servers**: Block servers are responsible for uploading blocks to cloud storage. Block storage, also known as block-level storage, is a technology used to store data files in cloud-based environments. Files can be split into multiple blocks, each with a unique hash value, and these blocks are stored in our metadata database. Each block is treated as an independent object and is stored in our storage system. To reconstruct a file, blocks are assembled in a specific order. We follow the block size reference from Dropbox, which sets the maximum block size to 4MB.

- **Cloud Storage**: Files are divided into smaller blocks and stored in cloud storage.

- **Cold Storage**: Cold storage is a specialized computer system designed for storing inactive data, which means files that have not been accessed for an extended period.

- **Load Balancer**:A load balancer evenly distributes requests among API servers.

- **API Servers**: These servers handle various tasks, excluding the uploading process. API servers are responsible for user authentication, managing user profiles, updating file metadata, and more.

- **Metadata Database**: This database stores metadata related to users, files, blocks, versions, and other relevant information. It's important to note that actual file data is stored in the cloud, while the metadata database contains metadata only.

- **Metadata Cache**: Some of the metadata is cached to enable fast retrieval.

- **Notification Service**: The notification service operates as a publisher/subscriber system, facilitating the transfer of data from the service to clients when specific events occur. In our case, the notification service notifies relevant clients when a file is added, edited, or removed elsewhere, enabling them to pull the latest changes.

- **Offline Backup Queue**: When a client is offline and cannot retrieve the latest file changes, the offline backup queue stores this information so that changes can be synchronized when the client is back online.




### Technology Stack and Communication Patterns

**API Gateway (Python)**:

- **Programming Language**: Python is a suitable choice for building the API Gateway due to its simplicity and a variety of libraries and frameworks available for creating RESTful APIs.
- **Framework**: I  will use a lightweight framework like Flask building the API Gateway.
- **Communication Pattern**: Since the API Gateway serves as the entry point for incoming requests and handles load balancing, security, and routing, I will primarily use RESTful APIs for synchronous communication with the underlying microservices.

**Service Discovery (Python):**

- **Programming Language**: Python is a suitable choice for implementing Service Discovery due to its simplicity and compatibility with microservices architectures.
- **Communication Pattern**: Service Discovery primarily uses synchronous communication to register and deregister microservices and maintain an up-to-date list of available services. RESTful APIs can be used for communication between the Service Discovery component and microservices.

**Block Servers**
- **Programming Languages**: C# for the block server logic.
- **Frameworks**: ASP.NET Core (C#) for building the block server API.
- **Communication Pattern**: RESTful APIs (synchronous communication) for interaction with other components.

**Cloud Storage**
- I will utilize cloud storage services like Azure Blob Storage to store the file blocks. These services provide reliable, scalable, and secure storage for your system.

**Cold Storage**
- I will use the cold storage options provided by cloud providers like Azure Archive Storage for inactive data.

**API Servers**
- **Programming Languages**: C# (ASP.NET Core) for the API server logic.
- **Frameworks**: ASP.NET Core for C#.
- **Communication Pattern**: RESTful APIs (synchronous communication) for interaction with clients.

**Metadata Database**
- **Database**: I will use a relational database like Microsoft SQL Server.

**Metadata Cache**
- **Caching Framework**: Redis for caching frequently accessed metadata.

**Notification Service**
- **Programming Languages**: C#.
- **Frameworks**: ASP.NET Core (C#).
- **Communication Pattern**: Message queues (asynchronous communication) such as RabbitMQ or to handle notifications efficiently.


**Offline Backup Queue**
- **Message Queue**: I will use the same message queue system RabbitMQ  as the Notification Service to handle offline backup tasks.


### Design Data Management 

#### Endpoints and Data Formats

#### API Gateway Endpoints

1. **Endpoint: `/api/register` (POST)**
   - **Data Format (Request):**
     ```json
     {
       "username": "user123",
       "password": "secure_password"
     }
     ```
   - **Response:**
     ```json
     {
       "message": "Registration successful",
       "user_id": "12345"
     }
     ```

2. **Endpoint: `/api/login` (POST)**
   - **Data Format (Request):**
     ```json
     {
       "username": "user123",
       "password": "secure_password"
     }
     ```
   - **Response (Success):**
     ```json
     {
       "message": "Login successful",
       "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
     }
     ```
   - **Response (Failure):**
     ```json
     {
       "error": "Invalid credentials"
     }
     ```

#### Block Server Endpoints

1. **Endpoint: `/api/upload` (POST)**
   - **Data Format (Request):**
     - File block data (binary)
   - **Response:**
     ```json
     {
       "message": "Block uploaded successfully",
       "block_id": "b12345"
     }
     ```

#### API Servers Endpoints

1. **Endpoint: `/api/files/upload` (POST)**
   - **Data Format (Request):**
     - File data (binary)
   - **Response:**
     ```json
     {
       "message": "File uploaded successfully",
       "file_id": "f12345",
       "file_url": "https://yourfilestorage.com/f12345"
     }
     ```

2. **Endpoint: `/api/files/download/{file_id}` (GET)**
   - **Data Format (Request):**
     - No data required.
   - **Response (Success):**
     - Binary data of the requested file.
   - **Response (Failure):**
     ```json
     {
       "error": "File not found"
     }
     ```

3. **Endpoint: `/api/files/list` (GET)**
   - **Data Format (Request):**
     - No data required.
   - **Response:**
     ```json
     {
       "files": [
         {
           "file_id": "f12345",
           "file_name": "example.txt",
           "file_size": "1024 KB",
           "upload_date": "2023-09-20T12:00:00Z"
         },
         {
           "file_id": "f67890",
           "file_name": "image.jpg",
           "file_size": "5120 KB",
           "upload_date": "2023-09-21T10:30:00Z"
         }
       ]
     }
     ```

4. **Endpoint: `/api/files/delete/{file_id}` (DELETE)**
   - **Data Format (Request):**
     - No data required.
   - **Response:**
     ```json
     {
       "message": "File deleted successfully"
     }
     ```
 


### Deployment and Scaling Strategy

In my File Sharing System project, I will implement a robust deployment and scaling strategy using Docker and Kubernetes. This approach ensures flexibility, scalability, and resilience in our microservices architecture.

#### Containerization with Docker

1. **Containerization Benefits:**

   - **Isolation**: Each microservice is encapsulated in a Docker container, ensuring isolation of dependencies and configurations.

   - **Consistency**: Docker containers maintain consistency across development, testing, and production environments.

   - **Portability**: Containers are portable and can run on various platforms and cloud providers.

2. **Docker Image Creation:**

   - We create Dockerfiles for each microservice, specifying base images, dependencies, and configurations.

   - Docker images are built from these Dockerfiles and stored in a container registry (e.g., Docker Hub, Azure Container Registry).


#### Orchestration with Kubernetes

1. **Kubernetes Benefits:**

   - **Scaling**: Kubernetes enables horizontal scaling by replicating containers as needed.

   - **Self-Healing**: Automatic container restarts are performed by Kubernetes in case of failures.

   - **Load Balancing**: Kubernetes provides built-in load balancing for distributing traffic among containers.


