import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Job, JobWithFilledInfo, JobWithRoles } from './job';

@Injectable()
export class JobService {

  constructor(private http: HttpClient) {
  }

  list() {
    return this.http.get<Job[]>('/api/job');
  }

  listWithFilledInfo(){
    return this.http.get<JobWithFilledInfo[]>('/api/job/filled');
  }

  get(id: string) {
    return this.http.get<JobWithRoles>('/api/job/' + id);
  }

  save(job: Job) {
    return this.http.post<Job>('/api/job', job).toPromise();
  }

  delete(jobId: string) {
    return this.http.delete('/api/job/' + jobId, {responseType: 'text'});
  }
}
