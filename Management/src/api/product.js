import request from '@/utils/request';

export function getList(params) {
  return request({
    url: '/management/Product/GetList',
    method: 'get',
    params
  })
}

export function edit(data) {
    return request({
      url: '/management/Product/Edit',
      method: 'put',
      data,
      headers: {'Content-Type': 'application/json; charset=utf-8'}
    })
  }

export function add(data) {
    return request({
    url: '/management/Product/Create',
    method: 'post',
    data
    })
}

export function del(params) {
    return request({
    url: '/management/Product/Delete',
    method: 'delete',
    params
    })
}