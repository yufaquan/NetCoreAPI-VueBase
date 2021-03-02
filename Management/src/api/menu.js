import request from '@/utils/request'

export function getList(params) {
  return request({
    url: '/management/Menu/GetAllElementList',
    method: 'get',
    params
  })
}
