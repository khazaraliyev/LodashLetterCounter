# Description

LodashLetterCounter is a .NET application that connects to the GitHub API to analyze the lodash/lodash repository. It gathers statistics on the frequency of each letter present in the JavaScript/TypeScript files of the repository and outputs these statistics in decreasing order.

## Setup

### Prerequisites
.NET 8 SDK

Git

A GitHub Personal Access Token

### Clone the Repository:
```
git clone https://github.com/khazaraliyev/LodashLetterCounter.git

cd LodashLetterCounter
```
#### Build and Run the Application:

Open the solution in your preferred IDE (e.g., Visual Studio).
Restore dependencies and build the project.
Run the application. The application will prompt you to enter your GitHub token with 'repo' scope.
This token is required to authenticate requests to the GitHub API to avoid rate limits.

```
Please enter your GitHub token with 'repo' scope:
```
Enter your GitHub token and press Enter.

## Example Output
```
Count of letters:

e: 12345
a: 11234
r: 9988

...
```
