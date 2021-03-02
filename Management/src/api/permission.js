import request from '@/utils/request'

export function getroutes() {
  return request({
    url: '/management/Menu/GetHavePermissionsElementList',
    method: 'get'
    // ,baseURL:""
  })
}
