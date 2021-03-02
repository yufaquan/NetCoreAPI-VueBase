import Cookies from 'js-cookie'

const TokenKey = 'vue_admin_template_token'

export function getToken() {
  return Cookies.get(TokenKey)
}

export function setToken(token) {
  return Cookies.set(TokenKey, token)
}

export function removeToken() {
  return Cookies.remove(TokenKey)
}

export function getVisitToken(){
  return "FD68285087F9F68874B96F22756C8C56A940F45A390A9D18402DFC77EF115AA7404227657AB0AFCE531FECC16F56F1AC8470E099FAFFCCDCBE48F7F4E8496481E9251238DC3B86DE051ED16AB12A30E8B62134BECE3F94151B8D085FE8EA342CF3B8A335C98314D8FFC6A0BF17BFD4052AB82ABA8B6D78925C1FFA8F76CFEC0C";
}