import http from 'k6/http';

const url = 'http://localhost/api/ExamSystem/AddData';

export default function () {
  let data = {
	"studentId": 122,
	"courseId": 125	
  };

  let res = http.post(url, JSON.stringify(data), {
    headers: { 'Content-Type': 'application/json' },
  });
  console.log(res); 
}
