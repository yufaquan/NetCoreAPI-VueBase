import request from '@/utils/request';

export function get(params) {
  return request({
    url: '/management/Configuration/Get',
    method: 'get',
    params
  })
}

export function set(data) {
    return request({
      url: '/management/Configuration/Set',
      method: 'put',
      data
    })
  }
