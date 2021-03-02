import request from '@/utils/request';

export function getList(params) {
  return request({
    url: '/management/CancelCode/GetList',
    method: 'get',
    params
  })
}

export function cancel(params) {
    return request({
      url: '/management/CancelCode/Cancel',
      method: 'post',
      params
    })
  }

export function add(params) {
    return request({
    url: '/management/CancelCode/add',
    method: 'post',
    params
    })
}

export function del(params) {
    return request({
    url: '/management/CancelCode/delete',
    method: 'delete',
    params
    })
}