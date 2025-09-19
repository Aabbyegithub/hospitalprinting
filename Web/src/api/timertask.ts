import axios from '../common/axios';



export function getTimerTaskList(params: any) {
  return axios.get('/api/Task/GetTimerTaskList', { params });
}

export function getTimerTaskDetail(taskId: number) {
  return axios.get('/api/Task/GetTimerTaskDetail', { params: { taskId } });
}

export function addTimerTask(task: any) {
  return axios.post('/api/Task/AddTimerTask', task);
}

export function updateTimerTask(task: any) {
  return axios.post('/api/Task/UpdateTimerTask', task);
}

export function deleteTimerTask(taskId: number) {
  return axios.post('/api/Task/DeleteTimerTask', null, { params: { taskId } });
}

export function startTask() {
  return axios.post('/api/Task/StartTask', null );
}

export function stopTask() {
  return axios.post('/api/Task/StopTask', null);
}
export function AddTask(jobId: number,jobName:string,cronExpression:string) {
  return axios.post('/api/Task/AddJob', null, { params: { jobId, jobName,cronExpression} });
}

export function pauseJob(jobId: number) {
  return axios.post('/api/Task/PauseJob', null, { params: { jobId } });
}

export function resumeJob(jobId: number) {
  return axios.post('/api/Task/ResumeJob', null, { params: { jobId } });
}

export function removeJob(jobId: number) {
  return axios.post('/api/Task/RemoveJob', null, { params: { jobId } });
}
