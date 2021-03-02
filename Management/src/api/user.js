import request from '@/utils/request'

export function login(data) {
  return request({
    url: '/management/Login/LoginByPwd',
    method: 'post',
    data
  })
}

export function getInfo() {
  return request({
    url: '/management/user/info',
    method: 'get',
    params: {  }
  })
}

export function getList(params) {
  return request({
    url: '/management/User/GetList',
    method: 'get',
    params
  })
}

export function Add(data) {
  return request({
    url: '/management/User/Add',
    method: 'post',
    data
  })
}

export function logout() {
  return request({
    url: '/management/Login/LoginOut',
    method: 'get'
  })
}
