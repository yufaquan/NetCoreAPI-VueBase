import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/management/log/list',
    method: 'get',
    params
  })
}


export function getAPIList(params) {
  return request({
    url: '/management/Log/GetAPIList',
    method: 'get',
    params
  })
}

export function getEventList(params) {
  return request({
    url: '/management/Log/GetEventList',
    method: 'get',
    params
  })
}

export function getErrorList(params) {
  return request({
    url: '/management/Log/GetErrorList',
    method: 'get',
    params
  })
}