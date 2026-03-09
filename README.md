# Bucket – Goal Management Web Application

## Overview

**Bucket** is a web application designed to help users organize and achieve their goals. The system allows users to create and manage goals across different time frames, including weekly, monthly, yearly and lifetime goals (bucket list).

The platform focuses on simplicity and motivation by providing tools for goal tracking, structured breakdown of complex goals, collaboration with friends and automated reminders.

## Features

### Goal Management

- Create, edit, postpone and delete goals
- Organize goals by time period:
    - Weekly
    - Monthly
    - Yearly
    - Bucket List (lifetime goals)
- Track goal completion status

### Goal Breakdown

Large goals can be broken down into smaller sub-goals using AI to make them easier to accomplish.

- Add and remove sub-goals
- Structured progress tracking

### Collaboration

Users can work on goals together with friends.

- Invite friends
- Create shared goals
- Track each others progress

### Notifications

- Reminders for upcoming deadlines
- Notifications for shared goal progress and updates

### Statistics & Progress Tracking

The system provides visual feedback on user progress through charts and statistics.

- Goal completion statistics
- Progress tracking across different time periods
- Visual charts using Chart.js

**Social**

- Searching for other users
- Adding friends

**Goal History**

- browsing and filtering through past goals

## Technology Stack

### Backend

- **ASP.NET Core MVC**
- **C#**
- MS SQL database
- **AutoMapper**

### Frontend

- **HTML**
- **CSS**
- **JavaScript**

### Libraries & Integrations

- **Chart.js** – for visualizing goal statistics
- **OpenAI API** – for AI-powered goal breakdown suggestions

## System Architecture

The application follows the **MVC (Model–View–Controller)** architecture.

- **Models** handle data and business logic.
- **Views** display goal information and user interfaces.
- **Controllers** manage interactions between the user and the system.

AutoMapper is used to map domain models to view models.
