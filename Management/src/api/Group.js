import request from '@/utils/request';

export function getList(params) {
  return request({
    url: '/management/GroupBooking/GetList',
    method: 'get',
    params
  })
}

export function edit(data) {
    return request({
      url: '/management/GroupBooking/Edit',
      method: 'put',
      data
    })
  }

export function add(data) {
    return request({
    url: '/management/GroupBooking/Create',
    method: 'post',
    data
    })
}

export function del(params) {
    return request({
    url: '/management/GroupBooking/Delete',
    method: 'delete',
    params
    })
}