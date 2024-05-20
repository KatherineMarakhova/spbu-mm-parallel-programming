# Task4
## Docker
### Build
Run from solution folder (/Tasks):
```powershell
docker build -f task4\Dockerfile -t task4.webapi .
```
### Run
```powershell
docker run -it -p 80:80 -e SET_TYPE=StripedHashSet -e ASPNETCORE_ENVIRONMENT=Development task4.webapi
```
The following environment variables are supported:
- `ASPNETCORE_ENVIRONMENT` - application runtime environment. Available options are **Development** / **Staging** / **Production**.
- `SET_TYPE` - indicates internal concurrent set implementation. Options are **LazySet** (default) / **StripedHashSet**.

## Usage
Exam system should now be available on *localhost*, port *80*. Base API url: *http://localhost/api/ExamSystem*.

### Web API:
- **GET** `/api/ExamSystem/ContainData` (studentId, courseId) - check if credit present in the system
- **GET** `/GetAll` - get the current total number of credits
- **POST** `/api/ExamSystem/AddData` (studentId, courseId) - add new credit to the system
- **DELETE** `/api/ExamSystem/RemoveData` (studentId, courseId) - remove credit from the system

### Swagger:
Available only in **Development** environment at url: *http://localhost/swagger/index.html*.
