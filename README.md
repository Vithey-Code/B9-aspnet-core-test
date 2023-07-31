# How to run the application

There are two ways in order to run an application:
1. ### Run in Visual Studio
	Clone git@github.com:Vithey-Code/B9-aspnet-core-test.git and CD into B9-aspnet-core-test\Github folder
	and open Github.sln.
	There are two folders after you clone this project:
		1. Github: it is a main project
		2. Test: It is a test project
   So you can run the application included Github.csproj and Github.Test.csproj in Visual Studio 2022.
3. ### Run with Docker Compose
	 Please navigate to the same folder of docker-compse.yml file and then open the command prompt:
	 1. Build and run in the background
	 ```
	 docker-compose up -d --build
   ```
   	 2. To see the docker container running and also see the current running port
	 ```
	 docker ps
   ```
	 3. Open your web browser
	 ```
	 http://localhost:5000
   ```
 4. ### Result
    Navigate to GitHub Pull Request Menu
 ![b9-2](https://github.com/Vithey-Code/B9-aspnet-core-test/assets/114791690/db341eb6-7648-4ffa-9663-27b7108ac925)
    Wait a bit for running to get the data from Github API
 ![image](https://github.com/Vithey-Code/B9-aspnet-core-test/assets/114791690/1c1c042f-9c1e-4b13-8fbd-083906f9956a)
    As we are using anonymous API and Github also has an implementation rate limit, sometimes we are getting blocked in the request and I also alert the message to notify to the user.
 ![b9-4](https://github.com/Vithey-Code/B9-aspnet-core-test/assets/114791690/799499c8-6b55-4e1e-837b-8a0906406040)
  
5. ### Addition Information
   **Can’t search by labels**
   
    GitHub's API does not have a specific endpoint to search for pull requests by labels without authentication. The ability to search for pull requests by labels is typically restricted to authenticated users to prevent abuse.
    There are a few reasons why this is the case. First, it helps to protect the privacy of users' pull requests. If anyone could search for pull requests by labels without authentication, it would be possible to see pull requests that users did not want to be public.
    Second, it helps to prevent spam. If anyone could search for pull requests by labels without authentication, it would be possible to send spam messages to users who have open pull requests with certain labels.
    Finally, it helps to protect the integrity of GitHub's search results. If anyone could search for pull requests by labels without authentication, it would be possible to manipulate the search results to promote their own pull requests or to suppress the pull requests of their competitors.

   **Prevent heavy loading**

   To prevent heavy loading I also specify the page size and page number directly into the code but I don't expose it as a query parameter.

    **Data Not Appear**
  
     Based on our requirement we need to separate 3 objects containing ```active```, ```draft```, and ```stale```, but sometimes the live data from Github is updated we need to change pagination to get some old data to make   sure our login is working fine.
 


