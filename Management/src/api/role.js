import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/management/Role/GetList',
    method: 'get',
    params
  })
}
