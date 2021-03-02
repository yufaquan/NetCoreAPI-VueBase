import request from '@/utils/request';

export function getList(params) {
  return request({
    url: '/management/Order/GetList',
    method: 'get',
    params
  })
}

export function refund(params) {
  return request({
    url: '/wx/WxOpen/Refund',
    method: 'put',
    params
  })
}

export function edit(params) {
    return request({
      url: '/management/Order/Edit',
      method: 'put',
      params
    })
  }

export function add(params) {
    return request({
    url: '/management/Order/Create',
    method: 'post',
    params
    })
}

export function del(params) {
    return request({
    url: '/management/Order/Delete',
    method: 'delete',
    params
    })
}