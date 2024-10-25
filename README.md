# BaseProject

Welcome to BaseProject! This repository serves as the foundation for our solution, comprising four main projects: `Domain`, `Infrastructure`, `Application`, `WebAPI`, and `Test`.
It also includes Authentication and Authorization using `JWT`.

## Overview

BaseProject is structured to provide a modular and scalable architecture for building robust software solutions. Each project within this repository fulfills a specific role in our system:

- **Domain**: This layer does not depend on any other layer. This layer contains entities, enums, specifications etc.
  Add repository and unit of work contracts in this layer.
- **Infrastructure**: This layer contains database related logic (Repositories and DbContext), and third party library implementation (like logger and email service).
  This implementation is based on domain and application layer.
- **Application**: This layer contains business logic, services, service interfaces, request and response models.
  Third party service interfaces are also defined in this layer.
  This layer depends on domain layer.
- **WebAPI**: Offers a RESTful API interface to interact with our system. This project serves as the entry point for external clients and facilitates communication with our application.

- **Test**: The Test project within BaseProject is dedicated to ensuring the quality and reliability of our software solution.

## Getting Started

To start using BaseProject, follow these steps:

> **Front end:** This project using **npm**, you can start by command `npm run dev`
> There are four roles:
> User: user@gmail.com
> Staff: staff@gmail.com
> Owner: owner@gmail.com
> Admin: admin@gmail.com > **Common password: Password12@**
