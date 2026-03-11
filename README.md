## 🎮 Web Application for Project Management of Game Development Teams

A full-stack project management system built as part of a seminar project, following Larman’s Method of software development.
This README summarizes the technology stack, architecture, features, and provides an ER diagram of the underlying database.

## 📌 Overview

This project includes documentation and implementation of the first two phases of Larman’s iterative development methodology:

User Requirements Specification

Requirements Analysis

The application is designed to support the organization and planning of game-development teams through a rich set of project management tools.

## 🚀 Tech Stack

ASP.NET Core 8

Entity Framework Core 9

MS SQL Server

Repository Pattern

ASP.NET Identity (authentication & authorization)

## 🧩 Features

✅ Project Management

  - Create and manage projects
  
  - Define budgets, timelines, and genres
  
  - Assign project managers

✅ Task Management

   - Create tasks within projects
  
   - Set task priorities, statuses, deadlines

  - Assign tasks to users
  
  - Comment on tasks

✅ Team Management

  - Add members to projects
  
  - Define user roles within each project
  
  - Manage responsibilities

✅ Resource Management

  - Track hardware, software, and financial resources
  
  - Assign resources to users
  
  - Manage resource costs and availability

✅ Reporting

  - Overview of project progress
  
  - Timeline & status visibility
  
  - Basic tracking for time and workflow

## 🔐 User Roles

The system uses ASP.NET Identity with three defined roles:

Administrator

Project Manager

User

Each role has different permissions within the system.

## 🏗️ Architecture

The application uses clean separation of concerns:

Business Logic Layer

Data Access Layer (via Repository Pattern)

Presentation Layer (MVC Views & Controllers)

Development followed the Database-First approach — allowing practice in both SQL schema design and reverse engineering the EF Core model.
